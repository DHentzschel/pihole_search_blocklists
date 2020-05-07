# Pihole search blocklists
Search your pihole blocklists using a regex! Very useful when a certain url should be removed from the blocklists.  

## Documentation
### Arguments
| Switch | Internal ID | Description |  
|--|--|--|
| -i | inputFile | Input file containing a list of urls to download and search |
| -r | regex | Regular expression for finding matches at the files downloaded |
| -o | outputFile | Output file containing regex matches |

### Example
The following example will read the file named input.txt line by line and check if it is in a valid URL format.
If yes, the file will be downloaded and searches matches for regex "xn--".  
All matches will be written to the given output file path.

    pihole_search_blocklists.exe -i input.txt -o output.txt -r xn-- 

