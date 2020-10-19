# Persisting State

web app - volume for logs

- see logs written from container
- lost when container removed

web app - bind mount for logs

- see logs written from container
- retained with replacement

sql - bind mount for data folder

- custom startup script
- check for db files before loading
