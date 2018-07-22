# ImHasher
A global dotnet tool for comparing the similarity of two images.

# Installation
     dotnet tool install --global ImHasher --version 0.1.0-beta 

# Usage
    imhasher path_to_img1  path_to_img2
    
# Options
    imhasher
    
> ImHasher: A tool to determine similarity between two images.
>
> Usage: imhasher [-m {diff|avg}] [-t tolerance] [imgPath1] [imgPath2]
> Output: {true|false}
>
> Options:
>
>   -m: determines the method of comparison. 'diff' for difference method (default) and 'avg' for average method.
>
>   -t: the hamming distance tolerance. Defaults to 8.`
