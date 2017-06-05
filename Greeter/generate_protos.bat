set TOOLS_PATH=..\packages\Grpc.Tools.1.3.6\tools\windows_x64

%TOOLS_PATH%\protoc.exe protos\greeter.proto --csharp_out .\

%TOOLS_PATH%\protoc.exe protos\greeter.proto --grpc_out .\ --plugin=protoc-gen-grpc=%TOOLS_PATH%\grpc_csharp_plugin.exe

endlocal
