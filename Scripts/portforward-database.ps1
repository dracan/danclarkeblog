k port-forward $(k get po -o jsonpath="{range .items[*]}{@.metadata.name}{end}" -l name=postgres) 5432:5432