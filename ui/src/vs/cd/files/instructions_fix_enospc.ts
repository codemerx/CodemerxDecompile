//    Copyright CodeMerx 2020
//    This file is part of CodemerxDecompile.

//    CodemerxDecompile is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    CodemerxDecompile is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.

//    You should have received a copy of the GNU Affero General Public License
//    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.

export default () => `
# CodemerxDecompile is unable to work properly

This error indicates that the file watcher is running out of handles. The current limit can be viewed by running:
|||
cat /proc/sys/fs/inotify/max_user_watches
|||

The limit can be increased to its maximum (except on Arch based distros) by executing the following commands:
|||
sudo echo fs.inotify.max_user_watches=524288 >> /etc/sysctl.conf
sudo sysctl -p
|||

To increase the limit on Arch based distros execute the following commands:
|||
sudo echo fs.inotify.max_user_watches=16382 >> /etc/sysctl.d/99-inotify-max-watches.conf
sudo sysctl --system
|||


`.replace(/\|/g, '`');
