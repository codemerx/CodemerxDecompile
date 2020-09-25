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
define(['exports', '@grpc/grpc-js', './main_pb', './common_pb'], function (exports, grpc, main_pb, common_pb) {

function serialize_AddResolvedAssemblyRequest(arg) {
  if (!(arg instanceof main_pb.AddResolvedAssemblyRequest)) {
    throw new Error('Expected argument of type AddResolvedAssemblyRequest');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_AddResolvedAssemblyRequest(buffer_arg) {
  return main_pb.AddResolvedAssemblyRequest.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_CreateProjectRequest(arg) {
  if (!(arg instanceof main_pb.CreateProjectRequest)) {
    throw new Error('Expected argument of type CreateProjectRequest');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_CreateProjectRequest(buffer_arg) {
  return main_pb.CreateProjectRequest.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_CreateProjectResponse(arg) {
  if (!(arg instanceof main_pb.CreateProjectResponse)) {
    throw new Error('Expected argument of type CreateProjectResponse');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_CreateProjectResponse(buffer_arg) {
  return main_pb.CreateProjectResponse.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_DecompileTypeRequest(arg) {
  if (!(arg instanceof main_pb.DecompileTypeRequest)) {
    throw new Error('Expected argument of type DecompileTypeRequest');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_DecompileTypeRequest(buffer_arg) {
  return main_pb.DecompileTypeRequest.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_DecompileTypeResponse(arg) {
  if (!(arg instanceof main_pb.DecompileTypeResponse)) {
    throw new Error('Expected argument of type DecompileTypeResponse');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_DecompileTypeResponse(buffer_arg) {
  return main_pb.DecompileTypeResponse.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_Empty(arg) {
  if (!(arg instanceof common_pb.Empty)) {
    throw new Error('Expected argument of type Empty');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_Empty(buffer_arg) {
  return common_pb.Empty.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_GetAllTypeFilePathsRequest(arg) {
  if (!(arg instanceof main_pb.GetAllTypeFilePathsRequest)) {
    throw new Error('Expected argument of type GetAllTypeFilePathsRequest');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_GetAllTypeFilePathsRequest(buffer_arg) {
  return main_pb.GetAllTypeFilePathsRequest.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_GetAllTypeFilePathsResponse(arg) {
  if (!(arg instanceof main_pb.GetAllTypeFilePathsResponse)) {
    throw new Error('Expected argument of type GetAllTypeFilePathsResponse');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_GetAllTypeFilePathsResponse(buffer_arg) {
  return main_pb.GetAllTypeFilePathsResponse.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_GetAssemblyRelatedFilePathsRequest(arg) {
  if (!(arg instanceof main_pb.GetAssemblyRelatedFilePathsRequest)) {
    throw new Error('Expected argument of type GetAssemblyRelatedFilePathsRequest');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_GetAssemblyRelatedFilePathsRequest(buffer_arg) {
  return main_pb.GetAssemblyRelatedFilePathsRequest.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_GetAssemblyRelatedFilePathsResponse(arg) {
  if (!(arg instanceof main_pb.GetAssemblyRelatedFilePathsResponse)) {
    throw new Error('Expected argument of type GetAssemblyRelatedFilePathsResponse');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_GetAssemblyRelatedFilePathsResponse(buffer_arg) {
  return main_pb.GetAssemblyRelatedFilePathsResponse.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_GetMemberDefinitionPositionRequest(arg) {
  if (!(arg instanceof main_pb.GetMemberDefinitionPositionRequest)) {
    throw new Error('Expected argument of type GetMemberDefinitionPositionRequest');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_GetMemberDefinitionPositionRequest(buffer_arg) {
  return main_pb.GetMemberDefinitionPositionRequest.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_GetMemberReferenceMetadataRequest(arg) {
  if (!(arg instanceof main_pb.GetMemberReferenceMetadataRequest)) {
    throw new Error('Expected argument of type GetMemberReferenceMetadataRequest');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_GetMemberReferenceMetadataRequest(buffer_arg) {
  return main_pb.GetMemberReferenceMetadataRequest.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_GetMemberReferenceMetadataResponse(arg) {
  if (!(arg instanceof main_pb.GetMemberReferenceMetadataResponse)) {
    throw new Error('Expected argument of type GetMemberReferenceMetadataResponse');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_GetMemberReferenceMetadataResponse(buffer_arg) {
  return main_pb.GetMemberReferenceMetadataResponse.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_GetProjectCreationMetadataFromTypeFilePathRequest(arg) {
  if (!(arg instanceof main_pb.GetProjectCreationMetadataFromTypeFilePathRequest)) {
    throw new Error('Expected argument of type GetProjectCreationMetadataFromTypeFilePathRequest');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_GetProjectCreationMetadataFromTypeFilePathRequest(buffer_arg) {
  return main_pb.GetProjectCreationMetadataFromTypeFilePathRequest.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_GetProjectCreationMetadataFromTypeFilePathResponse(arg) {
  if (!(arg instanceof main_pb.GetProjectCreationMetadataFromTypeFilePathResponse)) {
    throw new Error('Expected argument of type GetProjectCreationMetadataFromTypeFilePathResponse');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_GetProjectCreationMetadataFromTypeFilePathResponse(buffer_arg) {
  return main_pb.GetProjectCreationMetadataFromTypeFilePathResponse.deserializeBinary(new Uint8Array(buffer_arg));
}

function serialize_Selection(arg) {
  if (!(arg instanceof main_pb.Selection)) {
    throw new Error('Expected argument of type Selection');
  }
  return Buffer.from(arg.serializeBinary());
}

function deserialize_Selection(buffer_arg) {
  return main_pb.Selection.deserializeBinary(new Uint8Array(buffer_arg));
}


var RpcDecompilerService = exports.RpcDecompilerService = {
  getAssemblyRelatedFilePaths: {
    path: '/RpcDecompiler/GetAssemblyRelatedFilePaths',
    requestStream: false,
    responseStream: false,
    requestType: main_pb.GetAssemblyRelatedFilePathsRequest,
    responseType: main_pb.GetAssemblyRelatedFilePathsResponse,
    requestSerialize: serialize_GetAssemblyRelatedFilePathsRequest,
    requestDeserialize: deserialize_GetAssemblyRelatedFilePathsRequest,
    responseSerialize: serialize_GetAssemblyRelatedFilePathsResponse,
    responseDeserialize: deserialize_GetAssemblyRelatedFilePathsResponse,
  },
  getProjectCreationMetadataFromTypeFilePath: {
    path: '/RpcDecompiler/GetProjectCreationMetadataFromTypeFilePath',
    requestStream: false,
    responseStream: false,
    requestType: main_pb.GetProjectCreationMetadataFromTypeFilePathRequest,
    responseType: main_pb.GetProjectCreationMetadataFromTypeFilePathResponse,
    requestSerialize: serialize_GetProjectCreationMetadataFromTypeFilePathRequest,
    requestDeserialize: deserialize_GetProjectCreationMetadataFromTypeFilePathRequest,
    responseSerialize: serialize_GetProjectCreationMetadataFromTypeFilePathResponse,
    responseDeserialize: deserialize_GetProjectCreationMetadataFromTypeFilePathResponse,
  },
  getAllTypeFilePaths: {
    path: '/RpcDecompiler/GetAllTypeFilePaths',
    requestStream: false,
    responseStream: false,
    requestType: main_pb.GetAllTypeFilePathsRequest,
    responseType: main_pb.GetAllTypeFilePathsResponse,
    requestSerialize: serialize_GetAllTypeFilePathsRequest,
    requestDeserialize: deserialize_GetAllTypeFilePathsRequest,
    responseSerialize: serialize_GetAllTypeFilePathsResponse,
    responseDeserialize: deserialize_GetAllTypeFilePathsResponse,
  },
  decompileType: {
    path: '/RpcDecompiler/DecompileType',
    requestStream: false,
    responseStream: false,
    requestType: main_pb.DecompileTypeRequest,
    responseType: main_pb.DecompileTypeResponse,
    requestSerialize: serialize_DecompileTypeRequest,
    requestDeserialize: deserialize_DecompileTypeRequest,
    responseSerialize: serialize_DecompileTypeResponse,
    responseDeserialize: deserialize_DecompileTypeResponse,
  },
  getMemberReferenceMetadata: {
    path: '/RpcDecompiler/GetMemberReferenceMetadata',
    requestStream: false,
    responseStream: false,
    requestType: main_pb.GetMemberReferenceMetadataRequest,
    responseType: main_pb.GetMemberReferenceMetadataResponse,
    requestSerialize: serialize_GetMemberReferenceMetadataRequest,
    requestDeserialize: deserialize_GetMemberReferenceMetadataRequest,
    responseSerialize: serialize_GetMemberReferenceMetadataResponse,
    responseDeserialize: deserialize_GetMemberReferenceMetadataResponse,
  },
  getMemberDefinitionPosition: {
    path: '/RpcDecompiler/GetMemberDefinitionPosition',
    requestStream: false,
    responseStream: false,
    requestType: main_pb.GetMemberDefinitionPositionRequest,
    responseType: main_pb.Selection,
    requestSerialize: serialize_GetMemberDefinitionPositionRequest,
    requestDeserialize: deserialize_GetMemberDefinitionPositionRequest,
    responseSerialize: serialize_Selection,
    responseDeserialize: deserialize_Selection,
  },
  addResolvedAssembly: {
    path: '/RpcDecompiler/AddResolvedAssembly',
    requestStream: false,
    responseStream: false,
    requestType: main_pb.AddResolvedAssemblyRequest,
    responseType: common_pb.Empty,
    requestSerialize: serialize_AddResolvedAssemblyRequest,
    requestDeserialize: deserialize_AddResolvedAssemblyRequest,
    responseSerialize: serialize_Empty,
    responseDeserialize: deserialize_Empty,
  },
  createProject: {
    path: '/RpcDecompiler/CreateProject',
    requestStream: false,
    responseStream: false,
    requestType: main_pb.CreateProjectRequest,
    responseType: main_pb.CreateProjectResponse,
    requestSerialize: serialize_CreateProjectRequest,
    requestDeserialize: deserialize_CreateProjectRequest,
    responseSerialize: serialize_CreateProjectResponse,
    responseDeserialize: deserialize_CreateProjectResponse,
  },
};

exports.RpcDecompilerClient = grpc.makeGenericClientConstructor(RpcDecompilerService);
});
