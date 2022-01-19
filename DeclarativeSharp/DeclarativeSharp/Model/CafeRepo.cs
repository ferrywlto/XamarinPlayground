using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DeclarativeSharp.Model {
    public class CafeRepo {

        public async Task<int> ExecuteCommand(Action<MaidCafeContext> cmd) {
            await using var ctx = new MaidCafeContext();
            cmd(ctx);
            return await ctx.SaveChangesAsync();
        }

        public async Task<int> Init() {
            var cafes = new List<Cafe>() {
                new() {
                    Name = "Maid Planet メイドプラネット",
                    Address = "香港九龍旺角豉油街24-26號福泰大廈11樓"
                },
                new() {
                    Name = "Maid Planet +",
                    Address = "香港九龍旺角彌敦道594-596號旺角新城6樓全層"
                }
            };

            await using var ctx = new MaidCafeContext();

            if (ctx.Cafes.Any()) {
                await ExecuteCommand(
                    context => {context.Cafes.RemoveRange(context.Cafes.ToList());});
            }

            return await ExecuteCommand(context => {
                context.Cafes.AddRangeAsync(cafes);
            });

        }
        public async Task<List<Cafe>> GetAll() {
            await using var ctx = new MaidCafeContext();
            return await ctx.Cafes.ToListAsync();
        }

        public async Task<int> Add(Cafe cafe) {
            await using var ctx = new MaidCafeContext();
            ctx.Add(cafe);
            return await ctx.SaveChangesAsync();
        }

        public async Task<int> Delete(Cafe cafe) {
            await using var ctx = new MaidCafeContext();
            ctx.Remove(cafe);

            return await ctx.SaveChangesAsync();
        }

        public async Task<int> Update(Cafe cafe) {
            await using var ctx = new MaidCafeContext();
            ctx.Remove(cafe);
            ctx.Cafes.Update(cafe);
            return await ctx.SaveChangesAsync();
        }
    }
}
