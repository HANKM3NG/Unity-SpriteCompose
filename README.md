# Unity-SpriteCompose
用于解决画地图时，多种地形贴图相连接，但又不想画出所有相连可能性图片素材的问题。

![Image text](https://github.com/HANKM3NG/Unity-SpriteCompose/blob/master/pics/readme_ref.png?raw=true)

* 由多张不同的Sprite图片，根据配置的Level层级高低，组合成一张新的图片
* 组合成的图片保存到Resources目录为图片文件
* 自动修改图片文件的属性：如TextureType = Sprite等
* 生成文件后自动刷新项目文件目录 AssetDatabase.Refresh ()
* 懒人专用功能1： Resources目录内，文件夹名包含Sprite的文件Type自动改为Sprite并自动修改其他相关设置
* 懒人专用功能2： Resources目录内，文件夹名包含Sprite的文件名后缀包含_multiple的文件自动改为multiple，并且自动切图、改名

