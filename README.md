# Distributed Application - Voting application

A simple distributed application running across multiple docker containers.

This solution uses Python, .Net, Node.js  with Redis for messaging and Postgres for storage.

The voting application only accepts one vote for the client browser. It does not register additional votes if a vote has already been submitted from the client.

## Architecture

* A front-end web app in Python (/vote) which lets you vote between two options
* A Redis instance which collects new votes
* A .NET Worker (/worker) which consume votes and stores them in a Postgres database
* A Postgres database instance
* A Node.js(/result) web app which shows the results of the voting in real time.


