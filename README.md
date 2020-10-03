ServerControlPanel
------------------

ServerControlPanel is a simple web panel to stop/start a server.

It also provides the optional ability to trigger automatic restarts if the server crashes (or otherwise stops externally to the panel).

## Status

The project is minimalist but fully functional and relatively clean.

## Notes

- There is a password attempt flood prevention system. Too many invalid passwords from one IP in a short period of time will result in the user being blocked temporarily.

## Setup

- Requires DotNetCore SDK 3.0 or higher.
- Designed to run on remote Linux servers.
- By default configured to open on port `8011`.
- Meant to sit behind a proxy server (such as Apache).
- Strongly recommended that the external access point for the panel is HTTPS only.

Make folder `config` in the server directory, and within it add the following files:

- `password_hash.txt` - fill with a valid server password hash. Connect to the webserver at page `/Home/HashGenerator` to generate a hash. Please use a very long and generally strong password.
- `status_check.sh` - fill with a bash script that will output `true` if server is running, and `false` if not.
- `stop.sh` - fill with a bash script that will stop the server immediately.
- `start.sh` - fill with a bash script that will start the server immediately.
- `restart.sh` - fill with a bash script that will restart the server immediately.
- `config.txt` - for other configuration (see example).

## Sample Scripts

Here are sample scripts for a simple screen-based server to be tracked.

`status_check.sh:`
```sh
#!/bin/bash

SCREEN_LIST=`screen -ls`

if [[ $SCREEN_LIST == *'screen_name'* ]]; then
    echo 'true'
else
    echo 'false'
fi
```

`stop.sh`:
```sh
#!/bin/bash

screen -S screen_name -X kill
```

`start.sh`:
```sh
#!/bin/bash

cd ~/my_server/
screen -dmS screen_name ./start.sh
```

`restart.sh`:
```sh
#!/bin/bash

./config/stop.sh
./config/start.sh
```

`config.txt`:
```
auto_check=true
check_rate=300
double_check=true
```
`auto_check`: 'true' means check server status and start it if it's stopped (except when 'stop' was intentionally triggered on the panel) and restarts if so.
`check_rate`: set to time (in seconds) between status checks.
`double_check`: 'true' means two failed status checks in a row (with check_rate time in between) required to trigger a restart (to avoid accidentally triggering a new restart mid pre-existing restart).

## License

ServerControlPanel is Copyright (C) 2019-2020 Alex "mcmonkey" Goodwin, All Rights Reserved.
