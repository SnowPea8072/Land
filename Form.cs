using CSR;
using System.Threading;
using static Land.Land;

namespace Land
{
    internal class Form
	{
		public delegate void ONSELECT(FormSelectEvent json);

		/// <summary>
		/// 注册回调
		/// </summary>
		public static MCCSAPI.EventCab callback;

		/// <summary>
		/// 发送一个简易表单
		/// </summary>
		/// <param name="uuid">玩家 uuid</param>
		/// <param name="title">表单标题</param>
		/// <param name="content">表单内容</param>
		/// <param name="buttons">表单按钮</param>
		/// <param name="function">回调函数</param>
		public static void sendSimpleForm(string uuid, string title, string content, string buttons, ONSELECT function)
		{
			uint id = 0;
			callback = (x) =>
			{
				var json = BaseEvent.getFrom(x) as FormSelectEvent;
				if (json.formid == id && json.selected != "null")
				{
                    mcapi.removeAfterActListener(EventKey.onFormSelect, callback);
					function(json);
					return false;
				}
				return true;
			};
            mcapi.addAfterActListener(EventKey.onFormSelect, callback);
			id = mcapi.sendSimpleForm(uuid, title, content, buttons);
		}

		/// <summary>
		/// 发送一个模式对话框
		/// </summary>
		/// <param name="uuid">玩家 uuid</param>
		/// <param name="title">表单标题</param>
		/// <param name="content">表单内容</param>
		/// <param name="button1">表单按钮</param>
		/// <param name="button2">表单按钮</param>
		/// <param name="function">回调函数</param>
		public static void sendModalForm(string uuid, string title, string content, string button1, string button2, ONSELECT function)
		{
			uint id = 0;
			callback = (x) =>
			{
				var json = BaseEvent.getFrom(x) as FormSelectEvent;
				if (json.formid == id && json.selected != "null")
				{
                    mcapi.removeAfterActListener(EventKey.onFormSelect, callback);
					function(json);
					return false;
				}
				return true;
			};
            mcapi.addAfterActListener(EventKey.onFormSelect, callback);
			id = mcapi.sendModalForm(uuid, title, content, button1, button2);
            Thread.Sleep(timeout);
            mcapi.releaseForm(id);
		}

		/// <summary>
		/// 发送一个自定义表单
		/// </summary>
		/// <param name="uuid">玩家 uuid</param>
		/// <param name="json">表单内容</param>
		/// <param name="function">回调函数</param>
		public static void sendCustomForm(string uuid, string str, ONSELECT function)
		{
			uint id = 0;
			callback = (x) =>
			{
				var json = BaseEvent.getFrom(x) as FormSelectEvent;
				if (json.formid == id && json.selected != "null")
				{
                    mcapi.removeAfterActListener(EventKey.onFormSelect, callback);
					function(json);
					return false;
				}
				return true;
			};
            mcapi.addAfterActListener(EventKey.onFormSelect, callback);
			id = mcapi.sendCustomForm(uuid, str);
		}
	}
}
