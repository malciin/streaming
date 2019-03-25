FROM node:11.6.0-alpine
WORKDIR /code

ADD package.json /code/package.json
RUN npm i

ADD . /code
CMD npm start