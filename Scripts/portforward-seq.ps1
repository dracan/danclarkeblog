Start-Process "http://localhost:3010"
k port-forward $(k get po -o jsonpath="{range .items[*]}{@.metadata.name}{end}" -l name=seq) 3010:80
