/**
 * @fileoverview gRPC-Web generated client stub for 
 * @enhanceable
 * @public
 */

// GENERATED CODE -- DO NOT EDIT!


/* eslint-disable */
// @ts-nocheck


import * as grpcWeb from 'grpc-web';

import {
  DecompileTypeRequest,
  DecompileTypeResponse,
  GetAllTypeFilePathsRequest,
  GetAllTypeFilePathsResponse} from './main_pb';

export class RpcDecompilerClient {
  client_: grpcWeb.AbstractClientBase;
  hostname_: string;
  credentials_: null | { [index: string]: string; };
  options_: null | { [index: string]: string; };

  constructor (hostname: string,
               credentials?: null | { [index: string]: string; },
               options?: null | { [index: string]: string; }) {
    if (!options) options = {};
    if (!credentials) credentials = {};
    options['format'] = 'text';

    this.client_ = new grpcWeb.GrpcWebClientBase(options);
    this.hostname_ = hostname;
    this.credentials_ = credentials;
    this.options_ = options;
  }

  methodInfoGetAllTypeFilePaths = new grpcWeb.AbstractClientBase.MethodInfo(
    GetAllTypeFilePathsResponse,
    (request: GetAllTypeFilePathsRequest) => {
      return request.serializeBinary();
    },
    GetAllTypeFilePathsResponse.deserializeBinary
  );

  getAllTypeFilePaths(
    request: GetAllTypeFilePathsRequest,
    metadata: grpcWeb.Metadata | null): Promise<GetAllTypeFilePathsResponse>;

  getAllTypeFilePaths(
    request: GetAllTypeFilePathsRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
               response: GetAllTypeFilePathsResponse) => void): grpcWeb.ClientReadableStream<GetAllTypeFilePathsResponse>;

  getAllTypeFilePaths(
    request: GetAllTypeFilePathsRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.Error,
               response: GetAllTypeFilePathsResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        new URL('/RpcDecompiler/GetAllTypeFilePaths', this.hostname_).toString(),
        request,
        metadata || {},
        this.methodInfoGetAllTypeFilePaths,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/RpcDecompiler/GetAllTypeFilePaths',
    request,
    metadata || {},
    this.methodInfoGetAllTypeFilePaths);
  }

  methodInfoDecompileType = new grpcWeb.AbstractClientBase.MethodInfo(
    DecompileTypeResponse,
    (request: DecompileTypeRequest) => {
      return request.serializeBinary();
    },
    DecompileTypeResponse.deserializeBinary
  );

  decompileType(
    request: DecompileTypeRequest,
    metadata: grpcWeb.Metadata | null): Promise<DecompileTypeResponse>;

  decompileType(
    request: DecompileTypeRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.Error,
               response: DecompileTypeResponse) => void): grpcWeb.ClientReadableStream<DecompileTypeResponse>;

  decompileType(
    request: DecompileTypeRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.Error,
               response: DecompileTypeResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        new URL('/RpcDecompiler/DecompileType', this.hostname_).toString(),
        request,
        metadata || {},
        this.methodInfoDecompileType,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/RpcDecompiler/DecompileType',
    request,
    metadata || {},
    this.methodInfoDecompileType);
  }

}

