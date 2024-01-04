/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { Emitter, Event } from 'vs/base/common/event';
import { IDisposable, toDisposable, DisposableStore, DisposableMap } from 'vs/base/common/lifecycle';
import { URI, UriComponents } from 'vs/base/common/uri';
import { IFileWriteOptions, FileSystemProviderCapabilities, IFileChange, IFileService, IStat, IWatchOptions, FileType, IFileOverwriteOptions, IFileDeleteOptions, IFileOpenOptions, FileOperationError, FileOperationResult, FileSystemProviderErrorCode, IFileSystemProviderWithOpenReadWriteCloseCapability, IFileSystemProviderWithFileReadWriteCapability, IFileSystemProviderWithFileFolderCopyCapability, FilePermission, toFileSystemProviderErrorCode, IFileStatWithPartialMetadata, IFileStat } from 'vs/platform/files/common/files';
import { extHostNamedCustomer, IExtHostContext } from 'vs/workbench/services/extensions/common/extHostCustomers';
import { ExtHostContext, ExtHostFileSystemShape, IFileChangeDto, MainContext, MainThreadFileSystemShape } from '../common/extHost.protocol';
import { VSBuffer } from 'vs/base/common/buffer';
import { IMarkdownString } from 'vs/base/common/htmlContent';

@extHostNamedCustomer(MainContext.MainThreadFileSystem)
export class MainThreadFileSystem implements MainThreadFileSystemShape {

	private readonly _proxy: ExtHostFileSystemShape;
	private readonly _fileProvider = new DisposableMap<number, RemoteFileSystemProvider>();
	private readonly _disposables = new DisposableStore();

	constructor(
		extHostContext: IExtHostContext,
		@IFileService private readonly _fileService: IFileService
	) {
		this._proxy = extHostContext.getProxy(ExtHostContext.ExtHostFileSystem);

		const infoProxy = extHostContext.getProxy(ExtHostContext.ExtHostFileSystemInfo);

		for (const entry of _fileService.listCapabilities()) {
			infoProxy.$acceptProviderInfos(URI.from({ scheme: entry.scheme, path: '/dummy' }), entry.capabilities);
		}
		this._disposables.add(_fileService.onDidChangeFileSystemProviderRegistrations(e => infoProxy.$acceptProviderInfos(URI.from({ scheme: e.scheme, path: '/dummy' }), e.provider?.capabilities ?? null)));
		this._disposables.add(_fileService.onDidChangeFileSystemProviderCapabilities(e => infoProxy.$acceptProviderInfos(URI.from({ scheme: e.scheme, path: '/dummy' }), e.provider.capabilities)));
	}

	dispose(): void {
		this._disposables.dispose();
		this._fileProvider.dispose();
	}

	async $registerFileSystemProvider(handle: number, scheme: string, capabilities: FileSystemProviderCapabilities, readonlyMessage?: IMarkdownString): Promise<void> {
		this._fileProvider.set(handle, new RemoteFileSystemProvider(this._fileService, scheme, capabilities, readonlyMessage, handle, this._proxy));
	}

	$unregisterProvider(handle: number): void {
		this._fileProvider.deleteAndDispose(handle);
	}

	$onFileSystemChange(handle: number, changes: IFileChangeDto[]): void {
		const fileProvider = this._fileProvider.get(handle);
		if (!fileProvider) {
			throw new Error('Unknown file provider');
		}
		fileProvider.$onFileSystemChange(changes);
	}


	// --- consumer fs, vscode.workspace.fs

	$stat(uri: UriComponents): Promise<IStat> {
		return this._fileService.stat(URI.revive(uri)).then(stat => {
			return {
				ctime: stat.ctime,
				mtime: stat.mtime,
				size: stat.size,
				permissions: stat.readonly ? FilePermission.Readonly : undefined,
				type: MainThreadFileSystem._asFileType(stat)
			};
		}).catch(MainThreadFileSystem._handleError);
	}

	$readdir(uri: UriComponents): Promise<[string, FileType][]> {
		return this._fileService.resolve(URI.revive(uri), { resolveMetadata: false }).then(stat => {
			if (!stat.isDirectory) {
				const err = new Error(stat.name);
				err.name = FileSystemProviderErrorCode.FileNotADirectory;
				throw err;
			}
			return !stat.children ? [] : stat.children.map(child => [child.name, MainThreadFileSystem._asFileType(child)] as [string, FileType]);
		}).catch(MainThreadFileSystem._handleError);
	}

	private static _asFileType(stat: IFileStat | IFileStatWithPartialMetadata): FileType {
		let res = 0;
		if (stat.isFile) {
			res += FileType.File;

		} else if (stat.isDirectory) {
			res += FileType.Directory;
		}
		if (stat.isSymbolicLink) {
			res += FileType.SymbolicLink;
		}
		return res;
	}

	$readFile(uri: UriComponents): Promise<VSBuffer> {
		return this._fileService.readFile(URI.revive(uri)).then(file => file.value).catch(MainThreadFileSystem._handleError);
	}

	$writeFile(uri: UriComponents, content: VSBuffer): Promise<void> {
		return this._fileService.writeFile(URI.revive(uri), content)
			.then(() => undefined).catch(MainThreadFileSystem._handleError);
	}

	$rename(source: UriComponents, target: UriComponents, opts: IFileOverwriteOptions): Promise<void> {
		return this._fileService.move(URI.revive(source), URI.revive(target), opts.overwrite)
			.then(() => undefined).catch(MainThreadFileSystem._handleError);
	}

	$copy(source: UriComponents, target: UriComponents, opts: IFileOverwriteOptions): Promise<void> {
		return this._fileService.copy(URI.revive(source), URI.revive(target), opts.overwrite)
			.then(() => undefined).catch(MainThreadFileSystem._handleError);
	}

	$mkdir(uri: UriComponents): Promise<void> {
		return this._fileService.createFolder(URI.revive(uri))
			.then(() => undefined).catch(MainThreadFileSystem._handleError);
	}

	$delete(uri: UriComponents, opts: IFileDeleteOptions): Promise<void> {
		return this._fileService.del(URI.revive(uri), opts).catch(MainThreadFileSystem._handleError);
	}

	private static _handleError(err: any): never {
		if (err instanceof FileOperationError) {
			switch (err.fileOperationResult) {
				case FileOperationResult.FILE_NOT_FOUND:
					err.name = FileSystemProviderErrorCode.FileNotFound;
					break;
				case FileOperationResult.FILE_IS_DIRECTORY:
					err.name = FileSystemProviderErrorCode.FileIsADirectory;
					break;
				case FileOperationResult.FILE_PERMISSION_DENIED:
					err.name = FileSystemProviderErrorCode.NoPermissions;
					break;
				case FileOperationResult.FILE_MOVE_CONFLICT:
					err.name = FileSystemProviderErrorCode.FileExists;
					break;
			}
		} else if (err instanceof Error) {
			const code = toFileSystemProviderErrorCode(err);
			if (code !== FileSystemProviderErrorCode.Unknown) {
				err.name = code;
			}
		}

		throw err;
	}

	$ensureActivation(scheme: string): Promise<void> {
		return this._fileService.activateProvider(scheme);
	}


}

class RemoteFileSystemProvider implements IFileSystemProviderWithFileReadWriteCapability, IFileSystemProviderWithOpenReadWriteCloseCapability, IFileSystemProviderWithFileFolderCopyCapability {

	private readonly _onDidChange = new Emitter<readonly IFileChange[]>();
	private readonly _registration: IDisposable;

	readonly onDidChangeFile: Event<readonly IFileChange[]> = this._onDidChange.event;

	readonly capabilities: FileSystemProviderCapabilities;
	readonly onDidChangeCapabilities: Event<void> = Event.None;

	constructor(
		fileService: IFileService,
		scheme: string,
		capabilities: FileSystemProviderCapabilities,
		public readonly readOnlyMessage: IMarkdownString | undefined,
		private readonly _handle: number,
		private readonly _proxy: ExtHostFileSystemShape
	) {
		this.capabilities = capabilities;
		this._registration = fileService.registerProvider(scheme, this);
	}

	dispose(): void {
		this._registration.dispose();
		this._onDidChange.dispose();
	}

	watch(resource: URI, opts: IWatchOptions) {
		const session = Math.random();
		this._proxy.$watch(this._handle, session, resource, opts);
		return toDisposable(() => {
			this._proxy.$unwatch(this._handle, session);
		});
	}

	$onFileSystemChange(changes: IFileChangeDto[]): void {
		this._onDidChange.fire(changes.map(RemoteFileSystemProvider._createFileChange));
	}

	private static _createFileChange(dto: IFileChangeDto): IFileChange {
		return { resource: URI.revive(dto.resource), type: dto.type };
	}

	// --- forwarding calls

	stat(resource: URI): Promise<IStat> {
		return this._proxy.$stat(this._handle, resource).then(undefined, err => {
			throw err;
		});
	}

	readFile(resource: URI): Promise<Uint8Array> {
		return this._proxy.$readFile(this._handle, resource).then(buffer => buffer.buffer);
	}

	writeFile(resource: URI, content: Uint8Array, opts: IFileWriteOptions): Promise<void> {
		return this._proxy.$writeFile(this._handle, resource, VSBuffer.wrap(content), opts);
	}

	delete(resource: URI, opts: IFileDeleteOptions): Promise<void> {
		return this._proxy.$delete(this._handle, resource, opts);
	}

	mkdir(resource: URI): Promise<void> {
		return this._proxy.$mkdir(this._handle, resource);
	}

	readdir(resource: URI): Promise<[string, FileType][]> {
		return this._proxy.$readdir(this._handle, resource);
	}

	rename(resource: URI, target: URI, opts: IFileOverwriteOptions): Promise<void> {
		return this._proxy.$rename(this._handle, resource, target, opts);
	}

	copy(resource: URI, target: URI, opts: IFileOverwriteOptions): Promise<void> {
		return this._proxy.$copy(this._handle, resource, target, opts);
	}

	open(resource: URI, opts: IFileOpenOptions): Promise<number> {
		return this._proxy.$open(this._handle, resource, opts);
	}

	close(fd: number): Promise<void> {
		return this._proxy.$close(this._handle, fd);
	}

	read(fd: number, pos: number, data: Uint8Array, offset: number, length: number): Promise<number> {
		return this._proxy.$read(this._handle, fd, pos, length).then(readData => {
			data.set(readData.buffer, offset);
			return readData.byteLength;
		});
	}

	write(fd: number, pos: number, data: Uint8Array, offset: number, length: number): Promise<number> {
		return this._proxy.$write(this._handle, fd, pos, VSBuffer.wrap(data).slice(offset, offset + length));
	}
}
