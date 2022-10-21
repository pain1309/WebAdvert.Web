 - https://martinjt.me/2020/03/08/deploying-net-core-to-linux-using-codedeploy/
 - bổ sung thêm IAM Role với các Policy là (AmazonEC2RoleforAWSCodeDeploy, AWSCodeDeployFullAccess, AmazonS3ReadOnlyAccess, AWSCodeDeployRole) vào Instances EC2
 - Compress-Archive 'D:\note\docs_knowledge\WebAdvert.Web\codedeploy\*' artifacts\codedeploy.zip
	=> câu lệnh nén tất cả tệp