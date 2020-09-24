// GENERATED CODE -- DO NOT EDIT!

// Original file comments:
//    Copyright CodeMerx 2020
//    This file is part of CodemerxDecompile.
//
//    CodemerxDecompile is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    CodemerxDecompile is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.
//
'use strict';

define(['exports', '@grpc/grpc-js', './manager_pb', './common_pb'], function (exports, grpc, manager_pb, common_pb) {

function serialize_Empty(arg) {
  if (!(arg instanceof common_pb.Empty)) {
    throw new Error('Expected argument of type Empty');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_Empty(buffer_arg) {
  return common_pb.Empty.deserializeBinary(new Uint8Array(buffer_arg));
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
    requestType: common_pb.Empty,
    responseType: manager_pb.GetServerStatusResponse,
    requestSerialize: serialize_Empty,
    requestDeserialize: deserialize_Empty,
    responseSerialize: serialize_GetServerStatusResponse,
    responseDeserialize: deserialize_GetServerStatusResponse,
  },
  shutdownServer: {
    path: '/RpcManager/ShutdownServer',
    requestStream: false,
    responseStream: false,
    requestType: common_pb.Empty,
    responseType: common_pb.Empty,
    requestSerialize: serialize_Empty,
    requestDeserialize: deserialize_Empty,
    responseSerialize: serialize_Empty,
    responseDeserialize: deserialize_Empty,
  },
};

exports.RpcManagerClient = grpc.makeGenericClientConstructor(RpcManagerService);
});
