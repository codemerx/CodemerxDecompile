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

import { CancellationToken } from 'vs/base/common/cancellation';
import { RpcDecompilerClient } from 'vs/cd/platform/proto/main_grpc_pb';
import { ISerializedSearchSuccess, ITextQuery, SerializableFileMatch } from 'vs/workbench/services/search/common/search';
import { IProgressCallback } from 'vs/workbench/services/search/node/rawSearchService';
import * as grpc from "@grpc/grpc-js";
import { SearchRequest, SearchResultResponse } from 'vs/cd/platform/proto/main_pb';

export class DecompilerSearchEngine {
	constructor(private readonly serviceUrl: string, private readonly query: ITextQuery) {
	}

	public search(progressCallback: IProgressCallback, token: CancellationToken): Promise<ISerializedSearchSuccess> {
		return new Promise<ISerializedSearchSuccess>((resolve, reject) => {
			const client = new RpcDecompilerClient(this.serviceUrl, grpc.credentials.createInsecure());

			const request = new SearchRequest();
			request.setQuery(this.query.contentPattern.pattern);

			const stream = client.search(request, {});

			stream.on('data', (searchResult: SearchResultResponse) => {
				const match = new SerializableFileMatch(searchResult.getFilepath());
				const highlightRange = searchResult.getHighlightrange();
				const searchResultId = searchResult.getId();

				match.addMatch({
					searchResultId,
					preview: {
						text: searchResult.getPreview(),
						matches: {
							startLineNumber: 0,
							startColumn: highlightRange?.getStartindex() ?? 0,
							endLineNumber: 0,
							endColumn: highlightRange?.getEndindex() ?? searchResult.getPreview().length
						}
					},
					ranges: {
						startLineNumber: searchResultId,
						startColumn: searchResultId,
						endLineNumber: searchResultId,
						endColumn: searchResultId
					}
				});

				progressCallback(match.serialize());
			});

			stream.on('end', () => resolve({
				limitHit: false,
				type: 'success'
			}));

			stream.on('error', err => reject(err));
		});
	}
}
