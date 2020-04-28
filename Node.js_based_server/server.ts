import EventSource from "eventsource";
import express from "express";

const port = process.argv[2] ?? 3103;
const app: express.Application = express();
const eventSourceInitDict: EventSource.EventSourceInitDict = {https: {rejectUnauthorized: false}, headers: {'Access-Control-Allow-Origin': 'https://localhost:44330' }};
const es = new EventSource("https://localhost:44330/my-see-emitter", eventSourceInitDict);

let version = 0;
let data = "";

es.onopen = (e) => {
  console.log(e);
};

es.onmessage = (e: MessageEvent) => {
  console.log(e.data);
};

app.get("/sse", function (req: any, res: any) {
  console.log;
});

app.get("/data", function (req: any, res: any) {
  res.set('Content-Type', 'text/plain');
  res.set('Access-Control-Allow-Origin', 'https://localhost:44330');
    res.send({data, version});
});

app.listen(port, () =>
  console.info(`Server is listening at port ${port}`)
);
