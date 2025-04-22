# datawedge-MAUI-SampleApp
Showing how to integrate Zebra Datawedge barcode readings into an existing .NET MAUI App. DW is sending readings via broadcast Intents and needs a specific profile setup which is part of this solution.

## Latest updates (Apr.2025)

### v1.5 - Free-form Image Capture profiles are supported 
- Refer to https://github.com/ZebraDevs/datawedge-MAUI-SampleApp/blob/155b927eceba9d8c935e755583cf3030f614ec23/datawedge-MAUI-SampleApp/Platforms/Android/DWIntentReceiver.cs#L133
- This new code section detects and decodes intents sent by a free-form image capture workflow.
- Decoded barcode values are printed on the screen
- The related captured image is saved to disk
- Validated on TC58 BSP 14.20.14U160 - DW 15.0.16
- Screenshots
  - ![image](https://github.com/user-attachments/assets/89b63b60-ca78-4afd-b85b-211a3ff0cb71)
  - ![image](https://github.com/user-attachments/assets/4179dcbc-12e6-490f-b0bf-7fba7eca9780)



### v1.4 - Profile import
- A "Import Profile" button is now available. By default, Profile0 is the default profile. By pushing the Import Profile button, a specific, asset-predefined profile gets imported and associated to this app.
- Check it's working with the "DW Active Profile", to see the currently set default profile.
- If you make changes to the "com.ndzl.dwmaui" profile, remind that a new import will overwrite your changes.
- Code behind the "Import Profile" button is credit by Laurent Trudu - Thanks Laurent!

- ![image](https://github.com/user-attachments/assets/6a9c1c93-c56e-4240-ad09-edd5eb7093a7)

- ![image](https://github.com/user-attachments/assets/591c52c7-9393-49d3-8125-729f04cc6884)




## Jan.2025's updates

- now running on .NET9, targeting API Level 35!
- ![image](https://github.com/user-attachments/assets/964d47d2-61e6-466a-b9fd-7570e0fc9f89)


See the Releases section of this repository to download the DW Profile0.
To install the profile, follow https://techdocs.zebra.com/datawedge/latest/guide/settings/#datawedgesettings 

Alternatively, you can define a DW Profile at runtime by means of the DW Intent APIs as explained here https://techdocs.zebra.com/datawedge/11-4/guide/api/ 

For NG Simulscan / Multibarcode, see here https://github.com/ZebraDevs/datawedge-MAUI-SampleApp/issues/4#issuecomment-2468257978 

Code v1.3 covers Multibarcode and was tested on TC58 Android 14 (DW 13.0.325)

Screenshots from the app v1.2 - Tested on TC53e, Android 13, DW 13.0.341

![image](https://user-images.githubusercontent.com/11386676/220946535-1da4975f-7434-45aa-ba6c-27285c55c547.png)

![image](https://github.com/user-attachments/assets/8b7e4c94-e477-448f-9a27-f60f5c55c9e7)

![image](https://cxnt48.com/author?ghMAUIdw) 
