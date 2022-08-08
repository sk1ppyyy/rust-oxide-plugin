# 🤖 Bookmarks + Login REST API

<p align = "center">
  <a href=""><img src="https://img.shields.io/badge/Nest.js-v9.0.7-0f80c1?style=flat&logo=nestjs" alt="" /></a>
  <a href=""><img src="https://img.shields.io/badge/Docker-v20.10.17-0f80c1?style=flat&logo=docker" alt="" /></a>
  <a href=""><img src="https://img.shields.io/badge/Prisma-v4.1.1-0f80c1?style=flat&logo=prisma"alt="" /></a>
  <a href=""><img src="https://img.shields.io/badge/Passport.js-v0.6.0-0f80c1?style=flat&logo=passport"alt="" /></a>
  <a href="https://github.com/Neptunsk1y/nestjs-bookmarks-api"><img src="https://github.com/discordjs/discord.js/actions/workflows/test.yml/badge.svg" alt="" /></a>
</p>

> Build a bookmarks API from scratch using Nest.js + Docker + PostgreSQL + Passport.js + Prisma + Pactum + Dotenv. Building a CRUD REST API with end-to-end tests using modern web development techniques.
## 📄 Requirements
1. Nest.js v9.0.7 or newer
2. Docker v20.10.17 or newer
3. Prisma v4.1.1 or newer

## 🚀 Getting Started

```sh
git clone https://github.com/Neptunsk1y/nestjs-bookmarks-api.git
cd nestjs-bookmarks-api
yarn
yarn db:dev:restart 
```

After installation finishes follow configuration instructions then run `yarn start:dev` to start the project.

## 🚀 New project on Nest.js

```sh
npm i -g @nestjs/cli
nest new project-name
```

## 🚀 Building and running in production mode

To create an optimised version of the app:

```bash
npm run build
```

You can run the newly built app with `npm run start`. This uses [sirv](https://github.com/lukeed/sirv), which is included in your package.json's `dependencies` so that the app will work when you deploy to platforms like [Heroku](https://heroku.com).

## 🚀 Start in testing mode

To run the e2e test, you need to register in the console:
```sh
yarn test:e2e
```

## 🤝 License

Sample and its code provided under MIT license, please see [LICENSE](/LICENSE). All third-party source code provided
under their own respective and MIT-compatible Open Source licenses.

Copyright (C) 2022, Mikhail Chikankov