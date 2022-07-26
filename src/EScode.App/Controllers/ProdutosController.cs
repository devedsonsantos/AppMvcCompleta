﻿using Microsoft.AspNetCore.Mvc;
using EScode.App.ViewModels;
using EScode.Business.Interfaces;
using AutoMapper;
using EScode.Business.Models;

namespace EScode.App.Controllers
{
    public class ProdutosController : BaseController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IMapper _mapper;

        public ProdutosController(IMapper mapper,
                                  IProdutoRepository produtoRepository,
                                  IFornecedorRepository fornecedorRepository)
        {
            _mapper = mapper;
            _produtoRepository = produtoRepository;
            _fornecedorRepository = fornecedorRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores()));
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);

            if (produtoViewModel == null)
                return NotFound();


            return View(produtoViewModel);
        }

        public async Task<IActionResult> Create()
        {
            var produtoViewModel = await PopularFornecedores(new ProdutoViewModel());

            return View(produtoViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProdutoViewModel produtoViewModel)
        {
            produtoViewModel = await PopularFornecedores(produtoViewModel);

            if (!ModelState.IsValid)
                return View(produtoViewModel);

            var imgPrefix = Guid.NewGuid() + "_";

            if (!await UploadArquivo(produtoViewModel.ImagemUpload, imgPrefix))
                return View(produtoViewModel);

            produtoViewModel.Imagem = imgPrefix + produtoViewModel.ImagemUpload.FileName;
            await _produtoRepository.Adicionar(_mapper.Map<Produto>(produtoViewModel));

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);
            if (produtoViewModel == null)
                return NotFound();

            return View(produtoViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProdutoViewModel produtoViewModel)
        {
            if (id != produtoViewModel.Id)
                return NotFound();

            var produtoAtualizar = await ObterProduto(id);
            produtoViewModel.Fornecedor = produtoAtualizar.Fornecedor;
            produtoViewModel.Imagem = produtoAtualizar.Imagem;
            if (!ModelState.IsValid)
                return View(produtoViewModel);

            if(produtoViewModel.ImagemUpload != null)
            {
                var imgPrefix = Guid.NewGuid() + "_";
                if (!await UploadArquivo(produtoViewModel.ImagemUpload, imgPrefix))
                    return View(produtoViewModel);

                produtoAtualizar.Imagem = imgPrefix + produtoViewModel.ImagemUpload.FileName;
            }

            produtoAtualizar.Nome = produtoViewModel.Nome;
            produtoAtualizar.Valor = produtoViewModel.Valor;
            produtoAtualizar.Descricao = produtoViewModel.Descricao;
            produtoAtualizar.Ativo = produtoViewModel.Ativo;

            await _produtoRepository.Atualizar(_mapper.Map<Produto>(produtoAtualizar));

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);

            if (produtoViewModel == null)
                return NotFound();

            return View(produtoViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);

            if (produtoViewModel == null)
                return NotFound();

            await _produtoRepository.Remover(id);

            return RedirectToAction(nameof(Index));
        }

        private async Task<ProdutoViewModel> ObterProduto(Guid id)
        {
            var produto = _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));
            produto.Fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());

            return produto;
        }

        private async Task<ProdutoViewModel> PopularFornecedores(ProdutoViewModel produto)
        {
            produto.Fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());

            return produto;
        }

        private async Task<bool> UploadArquivo(IFormFile file, string imgPrefix)
        {
            if (file.Length <= 0) return false;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgPrefix + file.FileName);

            if (System.IO.File.Exists(path))
            {
                ModelState.AddModelError(String.Empty, "Já existe um arquivo com este nome!");
                return false;
            }

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return true;
        }
    }
}
