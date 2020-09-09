// GENERATED CODE -- DO NOT EDIT!

'use strict';

define(['exports', 'grpc', './manager_pb'], function (exports, grpc, manager_pb) {

function serialize_GetServerStatusRequest(arg) {
  if (!(arg instanceof manager_pb.GetServerStatusRequest)) {
    throw new Error('Expected argument of type GetServerStatusRequest');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_GetServerStatusRequest(buffer_arg) {
  return manager_pb.GetServerStatusRequest.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_GetServerStatusResponse(arg) {
  if (!(arg instanceof manager_pb.GetServerStatusResponse)) {
    throw new Error('Expected argument of type GetServerStatusResponse');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_GetServerStatusResponse(buffer_arg) {
  return manager_pb.GetServerStatusResponse.deserializeBinary(new Uint8Array(buffer_arg));
}


var RpcManagerService = exports.RpcManagerService = {
  getServerStatus: {
    path: '/RpcManager/GetServerStatus',
    requestStream: false,
    responseStream: false,
    requestType: manager_pb.GetServerStatusRequest,
    responseType: manager_pb.GetServerStatusResponse,
    requestSerialize: serialize_GetServerStatusRequest,
    requestDeserialize: deserialize_GetServerStatusRequest,
    responseSerialize: serialize_GetServerStatusResponse,
    responseDeserialize: deserialize_GetServerStatusResponse,
  },
};

exports.RpcManagerClient = grpc.makeGenericClientConstructor(RpcManagerService);
});
