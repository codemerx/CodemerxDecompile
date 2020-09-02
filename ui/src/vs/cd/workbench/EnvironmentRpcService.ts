import { IMainProcessService } from 'vs/platform/ipc/electron-sandbox/mainProcessService';
import { createChannelSender } from 'vs/base/parts/ipc/common/ipc';
import { createDecorator } from 'vs/platform/instantiation/common/instantiation';

export const IEnvironmentRpcService = createDecorator<IEnvironmentRpcService>('environmentRpcService');

export interface IEnvironmentRpcService {
	readonly _serviceBrand: undefined;

	getTempDir(): Promise<string>;
}

export class EnvironmentRpcService {
	readonly _serviceBrand: undefined;

	constructor(@IMainProcessService mainProcessService: IMainProcessService) {
		return createChannelSender<IEnvironmentRpcService>(mainProcessService.getChannel('environment'));
	}
}
