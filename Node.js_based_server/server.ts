import EventSource from "eventsource";
import express from "express";
import SseStream from "ssestream";
import fetch from "node-fetch";
import https from "https";

const agent = new https.Agent({
  rejectUnauthorized: false,
});

const remote = process.argv[2] ?? "https://localhost:44330";
const port = 3103;

const app: express.Application = express();
const eventSourceInitDict: EventSource.EventSourceInitDict = {
  https: { rejectUnauthorized: false },
  headers: { "Access-Control-Allow-Origin": remote },
};

let version = 0;
let data = "";
let sseList = [];

const es = new EventSource(`${remote}/my-see-emitter`, eventSourceInitDict);

es.onopen = (e) => {
  console.log(e);
};

es.onmessage = (e: MessageEvent) => {
  console.log("message on event source");
  console.log(e);
  setData(e.data);
  sseList.map((sse) => sse.write(e));
};

es.onerror = (e: MessageEvent) => {
  console.log(new Date());
  console.log(e);
};

app.use(function (req: any, res: any, next: any) {
  res.set("Access-Control-Allow-Origin", remote);
  // res.set("Access-Control-Allow-Origin", "*");
  res.set("Access-Control-Allow-Headers", "*");

  next();
});

app.get("/sse", function (req, res) {
  console.log("request on /sse");

  const sse = new SseStream(req);
  sse.pipe(res);
  sseList.push(sse);
});

app.get("/data", function (req: any, res: any) {
  console.log("request on /data");

  res.set("Content-Type", "text/plain");
  res.send({ data, version });
});

app.post("/update", function (req: any, res: any) {
  console.log("posted on /update");
  var data = "";
  req.on("data", function (chunk) {
    data += chunk;
  });
  req.on("end", function () {
    fetch(`${remote}/update-receiver`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: data,
      agent,
    })
      .then((response) => {
        res.sendStatus(response.status);
      })
      .catch(() => {
        res.sendStatus(500);
      });
  });
});

function setData(rawData) {
  const messageData = JSON.parse(rawData);
  console.log("setting data", messageData);

  version = messageData.version;
  data = messageData.data;
}

fetch(`${remote}/data`, { agent })
  .then((res) => res.text())
  .then((text) => setData(text))
  .catch(() => console.error("could not fetch data from server"));

app.listen(port, () => console.info(`Server is listening at port ${port}, connecting to ${remote}`));
