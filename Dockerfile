# Download Docker image, Node.js running on Alpine
FROM node:alpine

# Make an app directory to hold the server files.
RUN mkdir /app

# Set the working directory to app.
WORKDIR /app

COPY ./package.json /app/package.json

# Install npm packages.
RUN yarn install

COPY src /app/src
COPY tsconfig.json /app/tsconfig.json

RUN yarn run build

# Expose port 80
EXPOSE 8008

# Start the server.
CMD npx nodemon dist/App.js
