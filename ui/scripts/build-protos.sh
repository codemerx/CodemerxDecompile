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
PROTO_OUT_PATH=./src/vs/cd/workbench/proto

protoc -I=$PROTO_PATH main.proto \
  --js_out=import_style=commonjs:$PROTO_OUT_PATH \
  --grpc-web_out=import_style=typescript,mode=grpcwebtext:$PROTO_OUT_PATH
