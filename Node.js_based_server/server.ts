import EventSource from "eventsource";
import express from "express";
import SseStream from "ssestream";
import fetch from "node-fetch";
import https from "https";

const agent = new https.Agent({
  rejectUnauthorized: false,
});

const port = process.argv[2] ?? 3103;
const app: express.Application = express();
const eventSourceInitDict: EventSource.EventSourceInitDict = {
  https: { rejectUnauthorized: false },
  headers: { "Access-Control-Allow-Origin": "https://localhost:44330" },
};
const es = new EventSource(
  "https://localhost:44330/my-see-emitter",
  eventSourceInitDict
);

let version = 0;
let data = "";
let sseList = [];


es.onopen = (e) => {
  console.log(e);
};

es.onmessage = (e: MessageEvent) => {
  console.log("message on event source");
  setData(e.data);
  sseList.map((sse) => sse.write(e));
};

es.onerror = (e: MessageEvent) => {
  console.log(new Date());
  console.log(e);
  console.log(new Error());

};

app.use(function (req: any, res: any, next: any) {
  res.set("Access-Control-Allow-Origin", "https://localhost:44330");
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
    fetch("https://localhost:44330/update-receiver", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: data,
      agent,
    });
  });
  res.send();
});

function setData(rawData) {
  const messageData = JSON.parse(rawData);
  console.log("setting data", messageData);

  version = messageData.version;
  data = messageData.data;
}

fetch("https://localhost:44330/data", { agent })
  .then((res) => res.text())
  .then((text) => setData(text))
  .catch(() => console.error("could not fetch data from server"));

app.listen(port, () => console.info(`Server is listening at port ${port}`));
