#!/usr/bin/env bash

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
cd $SCRIPT_DIR

docker run --rm -it -p 9222:9222 --cap-add=SYS_ADMIN justinribeiro/chrome-headless:stable