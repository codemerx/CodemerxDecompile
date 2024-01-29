//    Copyright CodeMerx 2020
//    This file is part of CodemerxDecompile.

//    CodemerxDecompile is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    CodemerxDecompile is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.

//    You should have received a copy of the GNU Affero General Public License
//    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.

import { Registry } from 'vs/platform/registry/common/platform';
import { IDecompilationService } from 'vs/cd/workbench/DecompilationService';
import { IWorkbenchContributionsRegistry, IWorkbenchContribution, Extensions as WorkbenchExtensions } from 'vs/workbench/common/contributions';
import { Disposable } from 'vs/base/common/lifecycle';
import { LifecyclePhase } from 'vs/workbench/services/lifecycle/common/lifecycle';
import { IProgressService, ProgressLocation } from 'vs/platform/progress/common/progress';
import { IFileService } from 'vs/platform/files/common/files';
import { IEditorService } from 'vs/workbench/services/editor/common/editorService';
import { VSBuffer } from 'vs/base/common/buffer';
import { URI } from 'vs/base/common/uri';

class DecompilationStateInitializer extends Disposable implements IWorkbenchContribution {

	constructor(@IFileService private readonly fileService: IFileService,
				@IEditorService private readonly editorService: IEditorService,
				@IDecompilationService private readonly decompilationService: IDecompilationService,
				@IProgressService private readonly progressService: IProgressService) {
		super();
		this.initialize();
	}

	private initialize() : void {
		this.progressService.withProgress({ location: ProgressLocation.Dialog, sticky: true }, async progress => {
			progress.report({ message: 'Restoring workspace...'});

			await this.decompilationService.restoreDecompilationContext();
			await this.ensureEditorTabDecompiledContentsAreUpToDate();
		});
	}

	private async ensureEditorTabDecompiledContentsAreUpToDate() : Promise<void> {
		const openedEditorUris = this.editorService.editors.reduce((editorPaths: URI[], editor) => {
			if (editor.resource?.fsPath) {
				editorPaths.push(editor.resource);
			}

			return editorPaths;
		}, []);

		for(const editorUri of openedEditorUris) {
			if (await this.decompilationService.shouldDecompileFile(editorUri.fsPath)) {
				const sourceCode = await this.decompilationService.decompileType(editorUri.fsPath);
				await this.fileService.writeFile(editorUri, VSBuffer.fromString(sourceCode));
			}
		}
	}
}

Registry.as<IWorkbenchContributionsRegistry>(WorkbenchExtensions.Workbench).registerWorkbenchContribution(DecompilationStateInitializer, LifecyclePhase.Restored);
