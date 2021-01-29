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

import * as os from 'os';
import * as ua from 'universal-analytics';
import { generateUuid } from 'vs/base/common/uuid';
import { IEnvironmentService } from 'vs/platform/environment/common/environment';

import { createDecorator } from 'vs/platform/instantiation/common/instantiation';
import { IProductService } from 'vs/platform/product/common/productService';
import { IStorageMainService } from 'vs/platform/storage/node/storageMainService';

export const IAnalyticsMainService = createDecorator<IAnalyticsMainService>('IAnalyticsMainService');

export interface IAnalyticsMainService {
	readonly _serviceBrand: undefined;

	trackEvent(category: string, action: string, label?: string, value?: string | number): Promise<void>;
	trackException(description: string): Promise<void>;
};

const CLIENT_ID_KEY_NAME = 'decompiler-client-id';
const TRACKING_ID = 'UA-134764144-2';

export class AnalyticsMainService implements IAnalyticsMainService {
	readonly _serviceBrand: undefined;

	private visitor: ua.Visitor | undefined;

	constructor(@IEnvironmentService environmentService: IEnvironmentService,
				@IStorageMainService private readonly storageService: IStorageMainService,
				@IProductService private readonly productService: IProductService) {
		if (environmentService.isBuilt) {
			(async () => {
				await this.storageService.initialize();
				await this.initializeAnalyticsClient(this.storageService);

				this.visitor?.pageview('/').send();
			})();
		}
	}

	public async trackEvent(category: string, action: string, label?: string, value?: string | number) : Promise<void> {
		this.visitor?.event({
			ec: category,
			ea: action,
			el: label,
			ev: value
		})
		.send();
	}

	public trackException(description: string): Promise<void> {
		return new Promise((resolve, reject) => {
			this.visitor?.exception(description, (err) => {
				if (err) {
					reject(err);
					return;
				}

				resolve();
			});
		});
	}

	private async initializeAnalyticsClient(storageService: IStorageMainService) : Promise<void> {
		const userId = this.ensureClientUniqueIdIsPresent(storageService);

		this.visitor = ua(TRACKING_ID, userId);
		this.visitor.set('cd1', this.productService.version);
		this.visitor.set('cd2', os.platform());
		this.visitor.set('cd3', os.arch());
		this.visitor.set('cd4', os.release());
	}

	private ensureClientUniqueIdIsPresent(storageService: IStorageMainService) : string {
		let userId = storageService.get(CLIENT_ID_KEY_NAME);

		if (!userId) {
			userId = generateUuid();
			storageService.store(CLIENT_ID_KEY_NAME, userId);
		}

		return userId;
	}
};
