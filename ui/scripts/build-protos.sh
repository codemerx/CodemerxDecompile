#!/bin/bash

BASEDIR=$(dirname "$0")
cd ${BASEDIR}/../

PROTO_PATH=../common/proto
PROTO_OUT_PATH=./src/vs/cd/services/proto

protoc -I=$PROTO_PATH main.proto \
  --js_out=import_style=commonjs:$PROTO_OUT_PATH \
  --grpc-web_out=import_style=typescript,mode=grpcwebtext:$PROTO_OUT_PATH
