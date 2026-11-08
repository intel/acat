========================================

  UCLA dynamic texture dataset

Antoni Chan, SVCL, UCSD, 11-07-2007
========================================


This is the dataset of video texture patches generated from the
UCLA dynamic texture dataset [2], and used in [1].  Each patch is 
48x48x75, and was cropped from the area of the video texture that 
contained the most motion, over all videos within the same class. 
The 4-fold training and test sets are also included.  


--- Contents ---
Files are MATLAB files:

ucla_patches.mat -- the 200 video patches (48x48x75)
ImageMaster.mat  -- the class, and image name for each patch
EvalSet.mat      -- the training and test sets


--- References ---
If you use this dataset, please reference these two papers:

[1] A. B. Chan and N. Vasconcelos
"Probabilistic Kernels for the Classification of Auto-regressive Visual Processes"
Proceedings of IEEE Conference on Computer Vision and Pattern Recognition,
San Diego, 2005.

[2] P. Saisan, G. Doretto, Y.Wu, and S. Soatto, 
"Dynamic texture recognition",
in IEEE Conf. CVPR, vol. 2, 2001, pp. 5863.

