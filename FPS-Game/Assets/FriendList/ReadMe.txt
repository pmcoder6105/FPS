Thanks for purchase Photon Friend List.
Version 1.5

Required:
Unity 2017.4++
Photon PUN 2.

If you are using Photon PUN Classic you can download the compatible package here: https://goo.gl/588xgD

Get Started:----------------------------------------------------------------------------
- Import the package in your project.
- In the scene that you wan to add the friend list, drag inside a Canvas the prefab "PhotonFriendList" located in
  FriendList -> Content -> Prefab -> PhotonFriendList.
- Edit the position of Panel to your needs and Ready!.

Required in scene to work:----------------------------------------------------------------

- For Friend List work, need to be connected to a photon lobby, which requires that the scene 
found a script responsible for this, the package contains a sample script (bl_AutoJoin.cs) 
with basics of how to connect to the lobby, but it is recommended that you use it only as reference.

- while scene is not connect FriendList will not be visible, when connect to Master / Lobby, 
this will be enable automatically.

- Also you need sure that in the script that you use to connect with photon, 
you must send the name of the player "PhotonNetwork.NickName" and "PhotonNetwork.UserID" before connecting, this is very important because if it does,
Friend List will not work correctly.

for send the UserId you use:
 PhotonNetwork.AuthValues.UserId = PhotonNetwork.NickName;

Some data:----------------------------------------------------------------------------------

- the button "Join room" appears when: a friend in the list is connected and successfully joined to a room.
- Currently, the friends list is stored in a 'PlayerPrefab' locally.

Contact / Support:-------------------------------------------------------------------------
Forum: http://lovattostudio.com/forum/index.php
Email: contact.lovattostudio@gmail.com
