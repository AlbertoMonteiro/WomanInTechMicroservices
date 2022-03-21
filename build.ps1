Write-Host "Construindo imagem serviço hospedagem"
docker image build . -f .\src\WomanInTechMicroservices.Hospedagem.Api\Dockerfile --tag hospedagem

Write-Host "Construindo imagem serviço voo"
docker image build . -f .\src\WomanInTechMicroservices.Voo.Api\Dockerfile --tag voo

Write-Host "Construindo imagem serviço pacote"
docker image build . -f .\src\WomanInTechMicroservices.Api\Dockerfile --tag pacote

Pop-Location