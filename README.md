This project is a test around different method of having a real-time update of a web client.

To run the main server just do 

```sh
(cd "C# Based server/RealTimeTest" && dotnet run)
```

Then if you want to see the proxy in action you must install its dependency:
```sh
(cd Node.js_based_server/ && npm i)
```
build it:
```sh
(cd Node.js_based_server/ && npm run build)
```
and run it:
```sh
(cd Node.js_based_server/ && npm start https://localhost:5001)
```