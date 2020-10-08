

v2

dockerfile
build bat

```
cd $env:docker4dotnet

ls

docker image build -t dak4dotnet/signup-web:02-02 `
  -f ./docker/02-02-packaging-pre-built-apps/signup-web/v2/Dockerfile .
```

- change one code file & restore happens again

v3

dockerfile

```
docker image build -t dak4dotnet/signup-web:02-03 `
  -f ./docker/02-02-packaging-pre-built-apps/signup-web/v3/Dockerfile .
```

- change code file - rebuild fast
- can't optimize as much as VS because no persistent cache

