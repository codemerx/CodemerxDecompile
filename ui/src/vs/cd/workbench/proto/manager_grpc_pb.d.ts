// GENERATED CODE -- DO NOT EDIT!

// package:
// file: manager.proto

import * as manager_pb from "./manager_pb";
import * as grpc from "grpc";

interface IRpcManagerService extends grpc.ServiceDefinition<grpc.UntypedServiceImplementation> {
  getServerStatus: grpc.MethodDefinition<manager_pb.GetServerStatusRequest, manager_pb.GetServerStatusResponse>;
}

export const RpcManagerService: IRpcManagerService;

export class RpcManagerClient extends grpc.Client {
  constructor(address: string, credentials: grpc.ChannelCredentials, options?: object);
  getServerStatus(argument: manager_pb.GetServerStatusRequest, callback: grpc.requestCallback<manager_pb.GetServerStatusResponse>): grpc.ClientUnaryCall;
  getServerStatus(argument: manager_pb.GetServerStatusRequest, metadataOrOptions: grpc.Metadata | grpc.CallOptions | null, callback: grpc.requestCallback<manager_pb.GetServerStatusResponse>): grpc.ClientUnaryCall;
  getServerStatus(argument: manager_pb.GetServerStatusRequest, metadata: grpc.Metadata | null, options: grpc.CallOptions | null, callback: grpc.requestCallback<manager_pb.GetServerStatusResponse>): grpc.ClientUnaryCall;
}
