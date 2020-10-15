/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { getPathFromAmdModule } from 'vs/base/common/amd';
import { CancellationToken } from 'vs/base/common/cancellation';
import { canceled } from 'vs/base/common/errors';
import { Event } from 'vs/base/common/event';
import { IDisposable } from 'vs/base/common/lifecycle';
import { URI as uri } from 'vs/base/common/uri';
import { getNextTickChannel } from 'vs/base/parts/ipc/common/ipc';
import { Client, IIPCOptions } from 'vs/base/parts/ipc/node/ipc.cp';
import { IConfigurationService } from 'vs/platform/configuration/common/configuration';
import { IDebugParams, IEnvironmentService } from 'vs/platform/environment/common/environment';
import { parseSearchPort, INativeEnvironmentService } from 'vs/platform/environment/node/environmentService';
import { IFileService } from 'vs/platform/files/common/files';
import { ILogService } from 'vs/platform/log/common/log';
import { FileMatch, IFileMatch, IFileQuery, IProgressMessage, IRawSearchService, ISearchComplete, ISearchConfiguration, ISearchProgressItem, ISearchResultProvider, ISerializedFileMatch, ISerializedSearchComplete, ISerializedSearchProgressItem, isSerializedSearchComplete, isSerializedSearchSuccess, ITextQuery, ISearchService, isFileMatch } from 'vs/workbench/services/search/common/search';
import { SearchChannelClient } from './searchIpc';
import { SearchService } from 'vs/workbench/services/search/common/searchService';
import { IInstantiationService } from 'vs/platform/instantiation/common/instantiation';
import { ITelemetryService } from 'vs/platform/telemetry/common/telemetry';
import { IExtensionService } from 'vs/workbench/services/extensions/common/extensions';
import { registerSingleton } from 'vs/platform/instantiation/common/extensions';
/* AGPL */
import { IGrpcService } from 'vs/cd/workbench/GrpcService';
import { IAnalyticsService } from 'vs/cd/workbench/AnalyticsService';
/* End AGPL */

export class LocalSearchService extends SearchService {
	constructor(
		@ITelemetryService telemetryService: ITelemetryService,
		@ILogService logService: ILogService,
		@IExtensionService extensionService: IExtensionService,
		@IFileService fileService: IFileService,
		@IEnvironmentService readonly environmentService: INativeEnvironmentService,
		@IInstantiationService readonly instantiationService: IInstantiationService
	) {
		super(telemetryService, logService, extensionService, fileService);

		this.diskSearch = instantiationService.createInstance(DiskSearch, !environmentService.isBuilt || environmentService.verbose, parseSearchPort(environmentService.args, environmentService.isBuilt));
	}
}

export class DiskSearch implements ISearchResultProvider {
	/* AGPL */
	private raw!: IRawSearchService;
	/* End AGPL */

	constructor(
		verboseLogging: boolean,
		searchDebug: IDebugParams | undefined,
		@ILogService private readonly logService: ILogService,
		@IConfigurationService private readonly configService: IConfigurationService,
		/* AGPL */
		@IGrpcService grpcService: IGrpcService,
		@IAnalyticsService private readonly analyticsService: IAnalyticsService
		/* End AGPL */
	) {
		/* AGPL */
		grpcService.getServiceUrl().then(grpcServiceUrl => {
		/* End AGPL */
			const timeout = this.configService.getValue<ISearchConfiguration>().search.maintainFileSearchCache ?
				Number.MAX_VALUE :
				60 * 60 * 1000;

			const opts: IIPCOptions = {
				serverName: 'Search',
				timeout,
				args: ['--type=searchService'],
				// See https://github.com/Microsoft/vscode/issues/27665
				// Pass in fresh execArgv to the forked process such that it doesn't inherit them from `process.execArgv`.
				// e.g. Launching the extension host process with `--inspect-brk=xxx` and then forking a process from the extension host
				// results in the forked process inheriting `--inspect-brk=xxx`.
				freshExecArgv: true,
				env: {
					AMD_ENTRYPOINT: 'vs/workbench/services/search/node/searchApp',
					PIPE_LOGGING: 'true',
					VERBOSE_LOGGING: verboseLogging,
					/* AGPL */
					CD_GRPC_URL: grpcServiceUrl
					/* End AGPL */
				},
				useQueue: true
			};

			if (searchDebug) {
				if (searchDebug.break && searchDebug.port) {
					opts.debugBrk = searchDebug.port;
				} else if (!searchDebug.break && searchDebug.port) {
					opts.debug = searchDebug.port;
				}
			}

			const client = new Client(
				getPathFromAmdModule(require, 'bootstrap-fork'),
				opts);

			const channel = getNextTickChannel(client.getChannel('search'));
			this.raw = new SearchChannelClient(channel);
		/* AGPL */
		});
		/* End AGPL */
	}

	textSearch(query: ITextQuery, onProgress?: (p: ISearchProgressItem) => void, token?: CancellationToken): Promise<ISearchComplete> {
		if (token && token.isCancellationRequested) {
			throw canceled();
		}

		/* AGPL */
		this.analyticsService.trackEvent('Search', 'Initiate');
		/* End AGPL */

		const event: Event<ISerializedSearchProgressItem | ISerializedSearchComplete> = this.raw.textSearch(query);

		return DiskSearch.collectResultsFromEvent(event, onProgress, token);
	}

	fileSearch(query: IFileQuery, token?: CancellationToken): Promise<ISearchComplete> {
		if (token && token.isCancellationRequested) {
			throw canceled();
		}

		let event: Event<ISerializedSearchProgressItem | ISerializedSearchComplete>;
		event = this.raw.fileSearch(query);

		const onProgress = (p: ISearchProgressItem) => {
			if (!isFileMatch(p)) {
				// Should only be for logs
				this.logService.debug('SearchService#search', p.message);
			}
		};

		return DiskSearch.collectResultsFromEvent(event, onProgress, token);
	}

	/**
	 * Public for test
	 */
	static collectResultsFromEvent(event: Event<ISerializedSearchProgressItem | ISerializedSearchComplete>, onProgress?: (p: ISearchProgressItem) => void, token?: CancellationToken): Promise<ISearchComplete> {
		let result: IFileMatch[] = [];

		let listener: IDisposable;
		return new Promise<ISearchComplete>((c, e) => {
			if (token) {
				token.onCancellationRequested(() => {
					if (listener) {
						listener.dispose();
					}

					e(canceled());
				});
			}

			listener = event(ev => {
				if (isSerializedSearchComplete(ev)) {
					if (isSerializedSearchSuccess(ev)) {
						c({
							limitHit: ev.limitHit,
							results: result,
							stats: ev.stats
						});
					} else {
						e(ev.error);
					}

					listener.dispose();
				} else {
					// Matches
					if (Array.isArray(ev)) {
						const fileMatches = ev.map(d => this.createFileMatch(d));
						result = result.concat(fileMatches);
						if (onProgress) {
							fileMatches.forEach(onProgress);
						}
					}

					// Match
					else if ((<ISerializedFileMatch>ev).path) {
						const fileMatch = this.createFileMatch(<ISerializedFileMatch>ev);
						result.push(fileMatch);

						if (onProgress) {
							onProgress(fileMatch);
						}
					}

					// Progress
					else if (onProgress) {
						onProgress(<IProgressMessage>ev);
					}
				}
			});
		});
	}

	private static createFileMatch(data: ISerializedFileMatch): FileMatch {
		const fileMatch = new FileMatch(uri.file(data.path));
		if (data.results) {
			// const matches = data.results.filter(resultIsMatch);
			fileMatch.results.push(...data.results);
		}
		return fileMatch;
	}

	clearCache(cacheKey: string): Promise<void> {
		return this.raw.clearCache(cacheKey);
	}
}

registerSingleton(ISearchService, LocalSearchService, true);
