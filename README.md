# KyleHyde
Tool for looking at files within the Nintendo DS games Hotel Dusk and Last Window.

Much of the format code was written between 2017-2020 and shelved due to the time taken to get Last Window's BRA animation files to uncompress and display. In March 2023 parts was hastily converted to .NET 6 and WPF to upload here. There may be some updates from time to time to fix up format support and the clunky interface.

# Example of usage
Initially:
- Already have Hotel Dusk or Last Window NDS extracted.
- Build/run the KyleHyde app.
- Press 'Old Interface'
- (Hotel Dusk) Drag and drop .wpf files to the top section.
- (Last Window) Drag and drop .pack files to the bottom section.
- Look in the directory that contains the app for a GT-HotelDusk or GT-LastWindow folder.
- Move the contents to the same location as the extracted NDS.
- Close 'Old Interface'

Then:
- Press either 'Hotel Dusk' or 'Last Window', select the base directory of the extracted+unpacked files.
- Select files on the left
- Depending on format support they'll appear on the right as image/text/binary (hex).
