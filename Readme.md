About containercp:
==================
Kubernetes / MicroK8s / kubectl cp do not support persistently modifying a pod / container image. the container file system is ephemeral and lost when the pod is shut down.  
However, in some scenarios it may be beneficial to make persistent changes to the container image "in the field" and without having to rebuild and redeploy the container image.  
containercp is an open-source C# tool that allow users to persistently copy files from the machine running containerd to any docker image hosted by containerd (this is done by modifying the containerd datastore)
The tool cuts down on the time it takes to replace a single file in the image (building and replacing the entire image takes longer), which can make development more efficient.  

Licensing:
==========
The containercp source code is licensed under LGPL 2.1  
