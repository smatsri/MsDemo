start "gateway" /D Gateway dotnet run --urls http://localhost:4000
start "front 1" /D Front dotnet run --urls http://localhost:4001


start "hello service 1" /D HelloService dotnet run --urls "http://localhost:4010;https://localhost:4011"
start "hello service 2" /D HelloService dotnet run --urls "http://localhost:4020;https://localhost:4021"
start "hello service 3" /D HelloService dotnet run --urls "http://localhost:4030;https://localhost:4031"