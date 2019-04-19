import os;
import shutil

for x in os.listdir('.'):
	if os.path.isdir(x) and '.' != x[0]:
		shutil.rmtree('{}/bin'.format(x), ignore_errors=True)
		shutil.rmtree('{}/obj'.format(x), ignore_errors=True)