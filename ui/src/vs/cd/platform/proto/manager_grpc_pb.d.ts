// GENERATED CODE -- DO NOT EDIT!

// package: 
// file: manager.proto

import * as manager_pb from "./manager_pb";
import * as common_pb from "./common_pb";
import * as grpc from "@grpc/grpc-js";

interface IRpcManagerService extends grpc.ServiceDefinition<grpc.UntypedServiceImplementation> {
  getServerStatus: grpc.MethodDefinition<common_pb.Empty, manager_pb.GetServerStatusResponse>;
  shutdownServer: grpc.MethodDefinition<common_pb.Empty, common_pb.Empty>;
}

export const RpcManagerService: IRpcManagerService;

export class RpcManagerClient extends grpc.Client {
  constructor(address: string, credentials: grpc.ChannelCredentials, options?: object);
  getServerStatus(argument: common_pb.Empty, callback: grpc.requestCallback<manager_pb.GetServerStatusResponse>): grpc.ClientUnaryCall;
  getServerStatus(argument: common_pb.Empty, metadataOrOptions: grpc.Metadata | grpc.CallOptions | null, callback: grpc.requestCallback<manager_pb.GetServerStatusResponse>): grpc.ClientUnaryCall;
  getServerStatus(argument: common_pb.Empty, metadata: grpc.Metadata | null, options: grpc.CallOptions | null, callback: grpc.requestCallback<manager_pb.GetServerStatusResponse>): grpc.ClientUnaryCall;
  shutdownServer(argument: common_pb.Empty, callback: grpc.requestCallback<common_pb.Empty>): grpc.ClientUnaryCall;
  shutdownServer(argument: common_pb.Empty, metadataOrOptions: grpc.Metadata | grpc.CallOptions | null, callback: grpc.requestCallback<common_pb.Empty>): grpc.ClientUnaryCall;
  shutdownServer(argument: common_pb.Empty, metadata: grpc.Metadata | null, options: grpc.CallOptions | null, callback: grpc.requestCallback<common_pb.Empty>): grpc.ClientUnaryCall;
}
