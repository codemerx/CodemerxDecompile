import { RpcDecompilerClient } from './proto/MainServiceClientPb';
import { HelloRequest } from './proto/main_pb';

const client = new RpcDecompilerClient('https://localhost:5001');

const func = () => {
	const request = new HelloRequest();
	request.setName("This is test name sent from vs code");
	client.sayHello(request, null, (err, response) => {
		if (err) {
			console.log(err);
			return;
		}
		console.log(response.getMessage());
	});
};

export {
	func
};
