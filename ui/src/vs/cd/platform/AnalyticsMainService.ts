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
};

const ANALYTICS_USER_ID_KEY = 'decompiler-user-id';
const ANALYTICS_TRACKING_ID = 'UA-134764144-2';

export class AnalyticsMainService implements IAnalyticsMainService {
	readonly _serviceBrand: undefined;

	private visitor: ua.Visitor | undefined;

	constructor(@IEnvironmentService environmentService: IEnvironmentService,
				@IStorageMainService private readonly storageService: IStorageMainService,
				@IProductService private readonly productService: IProductService) {
		if (environmentService.isBuilt) {
			(async () => {
				await this.initializeAnalyticsClient();

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

	private async initializeAnalyticsClient() : Promise<void> {
		await this.storageService.initialize();

		const userId = this.ensureUserUniqueIdIsPresent(this.storageService);

		this.visitor = ua(ANALYTICS_TRACKING_ID, userId);
		this.visitor.set('cd1', this.productService.version);
		this.visitor.set('cd2', os.platform());
		this.visitor.set('cd3', os.arch());
		this.visitor.set('cd4', os.release());
	}

	private ensureUserUniqueIdIsPresent(storageService: IStorageMainService) : string {
		let userId = storageService.get(ANALYTICS_USER_ID_KEY);

		if (!userId) {
			userId = generateUuid();
			storageService.store(ANALYTICS_USER_ID_KEY, userId);
		}

		return userId;
	}
};
