ServerControlPanel
------------------

ServerControlPanel is a simple web panel to stop/start a server.

## Setup

- Designed to run on remote Linux servers.
- By default configured to open on port `8011`.

Make folder `config` in the server directory, and within it add the following files:

- `password_hash.txt` - fill with a valid server password hash. Connect to the webserver at page `/Home/HashGenerator` to generate a hash.
- `status_check.sh` - fill with a bash script that will output `true` if server is running, and `false` if not.
- `stop.sh` - fill with a bash script that will stop the server immediately.
- `start.sh` - fill with a bash script that will start the server immediately.
- `restart.sh` - fill with a bash script that will restart the server immediately.

## License

ServerControlPanel is Copyright (C) 2019 Alex "mcmonkey" Goodwin, All Rights Reserved.
