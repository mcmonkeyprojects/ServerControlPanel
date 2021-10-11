ServerControlPanel
------------------

ServerControlPanel is a simple web panel to stop/start a server.

It also provides the optional ability to trigger automatic restarts if the server crashes (or otherwise stops externally to the panel).

## Status

The project is minimalist but fully functional and relatively clean.

## Notes

- There is a password attempt flood prevention system. Too many invalid passwords from one IP in a short period of time will result in the user being blocked temporarily.

## Setup

- Requires .NET 5.0 or higher.
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

-----

`config.txt`:
```
auto_check=true
check_rate=300
double_check=true
```

- `auto_check`: 'true' means check server status and start it if it's stopped (except when 'stop' was intentionally triggered on the panel) and restarts if so.
- `check_rate`: set to time (in seconds) between status checks.
- `double_check`: 'true' means two failed status checks in a row (with check_rate time in between) required to trigger a restart (to avoid accidentally triggering a new restart mid pre-existing restart).

-----

### Previous License

ServerControlPanel is Copyright (C) 2019-2021 Alex "mcmonkey" Goodwin, All Rights Reserved.

### Licensing pre-note:

This is an open source project, provided entirely freely, for everyone to use and contribute to.

If you make any changes that could benefit the community as a whole, please contribute upstream.

### The short of the license is:

You can do basically whatever you want, except you may not hold any developer liable for what you do with the software.

### The long version of the license follows:

The MIT License (MIT)

Copyright (c) 2021 Alex "mcmonkey" Goodwin

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
