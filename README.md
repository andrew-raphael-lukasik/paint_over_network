# EXPERIMENT: send textures via (unet) network, fast

Features:
- client can draw simple pictures
- client can send his picture to dedicated game server
- raw texture bytes are being looselessly compressed by 7zip library before sending
- compressed bytes are sliced into chunks
