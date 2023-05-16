# start the container stack
# (assumes the caller has permission to do this)
docker-compose up -d


# wait for the service to be ready
while ! curl --fail --silent --head http://172.29.111.95:8080; do
  sleep 1
done

# open the browser window
start "" http://172.29.111.95:8080