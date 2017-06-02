set TOOLS_PATH=packages\Grpc.Tools.1.0.1\tools\windows_x64

%TOOLS_PATH%\protoc.exe Greeter\protos\greeter.proto --csharp_out Greeter 

%TOOLS_PATH%\protoc.exe Greeter\protos\greeter.proto --grpc_out Greeter --plugin=protoc-gen-grpc=%TOOLS_PATH%\grpc_csharp_plugin.exe

%TOOLS_PATH%\protoc.exe Chat\protos\chat.proto --csharp_out Chat 

%TOOLS_PATH%\protoc.exe Chat\protos\chat.proto --grpc_out Chat --plugin=protoc-gen-grpc=%TOOLS_PATH%\grpc_csharp_plugin.exe

%TOOLS_PATH%\protoc.exe HelloWorldClient\helloworld.proto --csharp_out HelloWorldClient

%TOOLS_PATH%\protoc.exe HelloWorldClient\helloworld.proto  --grpc_out HelloWorldClient --plugin=protoc-gen-grpc=%TOOLS_PATH%\grpc_csharp_plugin.exe

endlocal
