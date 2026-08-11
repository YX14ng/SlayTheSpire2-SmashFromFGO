#!/bin/sh
# steamcmd con HOME aislado para que la sesión de Workshop sobreviva al cliente Steam.
#
# Por qué: el wrapper /usr/sbin/steamcmd guarda su login en ~/.steam/... — el MISMO árbol que
# usa el cliente Steam de escritorio, que reescribe config.vdf al abrirse y borra el token de
# steamcmd (pasó el 2026-08-11: token de la noche anterior eliminado a las 02:13). Con un HOME
# propio, steamcmd tiene su config.vdf privado y el cliente nunca lo toca.
#
# Login inicial (una sola vez, interactivo): tools/steamcmd_fgo.sh +login <usuario> +quit
# Después: tools/.steamcmd_path.txt apunta acá y workshop_upload.ps1 lo usa sin re-login.
set -eu
FGO_HOME="${STEAMCMD_FGO_HOME:-$HOME/.local/share/steamcmd-fgo}"
REAL_STEAMCMD="${STEAMCMD_FGO_BIN:-/usr/sbin/steamcmd}"
mkdir -p "$FGO_HOME"
HOME="$FGO_HOME" exec "$REAL_STEAMCMD" "$@"
