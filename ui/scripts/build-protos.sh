#    Copyright CodeMerx 2020
#    This file is part of CodemerxDecompile.

#    CodemerxDecompile is free software: you can redistribute it and/or modify
#    it under the terms of the GNU Affero General Public License as published by
#    the Free Software Foundation, either version 3 of the License, or
#    (at your option) any later version.

#    CodemerxDecompile is distributed in the hope that it will be useful,
#    but WITHOUT ANY WARRANTY; without even the implied warranty of
#    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
#    GNU Affero General Public License for more details.

#    You should have received a copy of the GNU Affero General Public License
#    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.

#!/bin/bash

BASEDIR=$(dirname "$0")
cd ${BASEDIR}/../

PROTO_PATH=../common/proto
PROTO_OUT_PATH=./src/vs/cd/platform/proto
PROTOC_GEN_TS_PATH=node_modules\\.bin\\protoc-gen-ts.cmd
PROTOC_GEN_GRPC_PATH=node_modules\\.bin\\grpc_tools_node_protoc_plugin.cmd

protoc -I=$PROTO_PATH common.proto main.proto manager.proto \
  --plugin=protoc-gen-ts=${PROTOC_GEN_TS_PATH} \
  --plugin=protoc-gen-grpc=${PROTOC_GEN_GRPC_PATH} \
  --js_out=import_style=commonjs:${PROTO_OUT_PATH} \
  --ts_out=service=grpc-node:${PROTO_OUT_PATH} \
  --grpc_out="${PROTO_OUT_PATH}"

sed -i -- 's/import \* as grpc from "grpc";/import \* as grpc from "@grpc\/grpc-js";/g' ${PROTO_OUT_PATH}/*.ts
