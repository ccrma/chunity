#!/bin/bash
cd ..
cat make_ios/srcs.txt | xargs -I % echo cp % ../Chunity\ Boilerplate/Assets/Chunity/Plugins/iOS
