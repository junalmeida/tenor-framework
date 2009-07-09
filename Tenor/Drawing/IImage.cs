using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace Tenor
{
	namespace Drawing
	{
		/// <summary>
		/// Contem uma lista de modos de redimensionamento.
		/// </summary>
		/// <remarks></remarks>
		public enum ResizeMode
		{
			/// <summary>
			/// Redimensiona sem considerar proporções
			/// </summary>
			/// <remarks></remarks>
			Stretch,
			/// <summary>
			/// Redimensiona proporcionalmente
			/// </summary>
			/// <remarks></remarks>
			Proportional,
			/// <summary>
			/// Redimensiona de forma centralizada cortando as sobras
			/// </summary>
			/// <remarks></remarks>
			Crop
		}
		
		public interface IImage
		{
			
			
			
			///' <summary>
			///' Redimensiona a imagem proporcionalmente por largura
			///' </summary>
			///' <param name="Width">Nova largura em pixels</param>
			///' <remarks></remarks>
			//Sub ResizeByWidth(ByVal Width As Integer)
			
			///' <summary>
			///' Redimensiona a imagem proporcionalmente por altura
			///' </summary>
			///' <param name="Height">Nova altura em pixels</param>
			///' <remarks></remarks>
			//Sub ResizeByHeight(ByVal Height As Integer)
			
			/// <summary>
			/// Redimensiona a imagem proporcionalmente
			/// </summary>
			/// <param name="Percent">Porcentagem para redimensionamento</param>
			/// <remarks></remarks>
			void ResizeByPercent(int Percent);
			
			
			///' <summary>
			///' Redimensiona a imagem
			///' </summary>
			///' <param name="Size">Estrutura com largura e altura definidas</param>
			///' <remarks></remarks>
			//Sub Resize(ByVal Size As System.Drawing.Size)
			
			/// <summary>
			/// Redimensiona a imagem
			/// </summary>
			/// <param name="Width">Nova largura em pixels</param>
			/// <param name="Height">Nova altura em pixels</param>
			/// <param name="Mode">Um dos modos de redimensionamento</param>
			/// <remarks></remarks>
			void Resize(int Width, int Height, ResizeMode Mode);
			
			///' <summary>
			///' Redimensiona a imagem respeitando as proporções e corta as sobras de forma a centralizar a imagem.
			///' </summary>
			///' <param name="width">Nova largura em pixels</param>
			///' <param name="height">Nova altura em pixels</param>
			///' <remarks></remarks>
			//Sub ResizeAndCrop(ByVal width As Integer, ByVal height As Integer)
			
			
			/// <summary>
			/// Adiciona uma imagem sobre a imagem atual.
			/// Use preferencialmente imagens PNG.
			/// </summary>
			/// <param name="FileName">Imagem a ser adicionada.</param>
			/// <param name="Position">Posição desejada.</param>
			/// <param name="Margin">Margem desejada</param>
			/// <remarks></remarks>
			void AddPicture(string FileName, Position Position, int Margin);
			
			
			/// <summary>
			/// Define se a imagem produzida será de baixa qualidade.
			/// </summary>
			/// <value></value>
			/// <returns></returns>
			/// <remarks></remarks>
			bool LowQuality{
				get;
				set;
			}
			
		}
	}
	
}
