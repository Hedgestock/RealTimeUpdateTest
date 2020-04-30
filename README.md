This project is a test around different method of having a real-time update of a web client.

## Usage

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

## Bibliography

### Libraries

#### Libraries used for the proxy

https://github.com/expressjs/express\
https://github.com/node-fetch/node-fetch\
https://github.com/EventSource/node-ssestream\
https://github.com/EventSource/eventsource\
https://github.com/Hedgestock/eventsource

#### Library used for the server

https://github.com/tpeczek/Lib.AspNetCore.ServerSentEvents

### Usefull links

#### OWASP Top 10
https://owasp.org/www-project-top-ten/

#### HTML living standard

https://html.spec.whatwg.org/multipage/server-sent-events.html#server-sent-events\
https://html.spec.whatwg.org/multipage/web-sockets.html#the-websocket-interface

#### MDN documentation

https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events\
https://developer.mozilla.org/en-US/docs/Web/API/WebSocket\
https://developer.mozilla.org/en-US/docs/Web/API/WebSockets_API
