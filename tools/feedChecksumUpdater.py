#!/usr/bin/env python
# vim: set fileencoding=UTF-8 :
#
# The MIT License (MIT)
#
# Copyright (c) 2014 The SharpFlame Authors.
#
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:

# The above copyright notice and this permission notice shall be included in
# all copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
# THE SOFTWARE.
#
# Usage:
# 
# $ ./feedChecksumUpdater.py -d <workingdir>
#

import getopt
import hashlib
import sys
import os.path

try:
	import xml.etree.cElementTree as ET
except ImportError:
	import xml.etree.ElementTree as ET

def hashfile(afile, hasher, blocksize=65536):
    buf = afile.read(blocksize)
    while len(buf) > 0:
        hasher.update(buf)
        buf = afile.read(blocksize)
    return hasher.hexdigest()

def updateFileUdateTask(workingDir, xmlElement):
	localPath = xmlElement.get('localPath')	
	if os.path.isfile(os.path.join(workingDir, localPath)):
		sha256 = hashfile(open(os.path.join(workingDir, localPath), 'rb'), hashlib.sha256())
		print "Updating", localPath, ", new hash =", sha256

		xmlElement.set('sha256-checksum', sha256)

		for elem in xmlElement:
			if elem.tag == "Conditions":
				for elem2 in elem:
					if elem2.tag == "FileChecksumCondition":
						if elem2.get("checksumType") == "sha256":
							elem2.set("checksum", sha256)
	else:
		xmlElement.set('sha256-checksum', "File does not exists.")

def main(argv=None):
	if argv is None:
		argv = sys.argv


	try:
		optlist, args = getopt.getopt(argv[1:], "d:")
	except getopt.GetoptError as err:
		print str(err) # will print something like "option -a not recognized"	

	workingDir = ""

	for o, a in optlist:
		if o == "-d":
			workingDir = a

	if not os.path.exists(workingDir):
		print "Workingdir \"" + workingDir + "\" does not exists."
		return 1

	feed = os.path.join(workingDir, "UpdateFeed.xml")
	print "Updating the feed", feed

	xmlTree = ET.parse(feed)
	xmlRoot = xmlTree.getroot()

	for child in xmlRoot:
		if child.tag == "Tasks":
			for task in child:
				if task.tag == "FileUpdateTask":
					updateFileUdateTask(workingDir, task)

		elif child.tag == "Updater":
			for task in child:
				if task.tag == "FileUpdateTask":
					updateFileUdateTask(workingDir, task)

	xmlTree.write(feed)

	return 0
					
if __name__ == "__main__":
	sys.exit(main())