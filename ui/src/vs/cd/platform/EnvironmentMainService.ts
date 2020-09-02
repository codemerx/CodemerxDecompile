import { createDecorator } from 'vs/platform/instantiation/common/instantiation';
import { tmpdir } from 'os';

export const IEnvironmentMainService = createDecorator<IEnvironmentMainService>('environmentMainService');

interface IEnvironmentMainService {
	readonly _serviceBrand: undefined;

	getTempDir() : Promise<string>;
};

export class EnvironmentMainService implements IEnvironmentMainService {
	readonly _serviceBrand: undefined;

	getTempDir(): Promise<string> {
		return Promise.resolve(tmpdir());
	}
};
